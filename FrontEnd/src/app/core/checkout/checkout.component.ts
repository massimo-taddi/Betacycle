import { Component, OnInit } from '@angular/core';
import { HttpUserAdminService } from '../../shared/services/http-user-admin.service';
import { Address } from '../../shared/models/Address';
import { CommonModule } from '@angular/common';
import { SalesOrderHeader } from '../../shared/models/SalesOrderHeader';
import { BasketService } from '../../shared/services/basket.service';
import { ShoppingCartItem } from '../../shared/models/ShoppingCartItem';
import { Product } from '../../shared/models/Product';
import { ProductService } from '../../shared/services/product.service';
import { concat, lastValueFrom } from 'rxjs';
import { CheckoutService } from '../../shared/services/checkout.service';
import { SalesOrderDetail } from '../../shared/models/SalesOrderDetail';
import { Route, Router } from '@angular/router';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit{
  addresses: Address []=[];
  salesOrderHeader: SalesOrderHeader = new SalesOrderHeader();
  basketItemsProductsMap: Map<ShoppingCartItem, Product> = new Map();
  subTotalPrice: number = 0;
  taxAmount: number = 0;
  dateArrival: Date = new Date(Date.now()) //getDay = mese, getDate = giorno, getFullYear = anno
  expectedTime: string = '';
  isCheckoutDisabled: boolean = false;

  constructor(private http: HttpUserAdminService, private shoppingCartService: BasketService, private productService: ProductService,
              private checkoutService: CheckoutService, private router: Router){}

  ngOnInit(): void {
    this.http.httpGetCustomerAddresses().subscribe({
      next: (data: Address[]) =>{this.addresses = data;
        if(!(data.length!=0))
          this.isCheckoutDisabled = true;
      },
      error: (err: Error) => {console.log(err)}
    })
    this.fillBasket();
  }

  private fillBasket() {
    this.shoppingCartService.getRemoteBasketItems().subscribe({
      next: (items: ShoppingCartItem[]) => {
        var basketItems = items;
        basketItems.forEach((item) => {
          this.productService.getProductById(item.productId).subscribe({
            next: (product: Product) => {
              this.basketItemsProductsMap.set(item, product);
              this.salesOrderHeader.subTotal += (product.listPrice * item.quantity)
            },
            error: (err: Error) => {
              console.log(err.message);
            }
          });
        });
      },
      error: (err: Error) => {
        console.log(err.message);
      }
    });
  }

  setShipAddress(name: string){
    const radios = document.querySelectorAll<HTMLInputElement>(`input[name="${name}"]:checked`);
    this.salesOrderHeader.shipToAddressId = this.addresses[radios[0].value as unknown as number].addressId;
  }
  setBillAddress(name: string){
    const radios = document.querySelectorAll<HTMLInputElement>(`input[name="${name}"]:checked`);
    this.salesOrderHeader.billToAddressId = this.addresses[radios[0].value as unknown as number].addressId;
  }
  
  setShipMethod(name: string){
    const radios = document.querySelectorAll<HTMLInputElement>(`input[name="${name}"]:checked`);
    this.dateArrival = new Date(Date.now())
    switch(radios[0].value as unknown as string){
      case "1": //UPS - 7
        this.salesOrderHeader.shipMethod = "UPS";
        this.dateArrival.setDate(this.dateArrival.getDate()+7);
        break;
      case "2": //FedEx - 10
        this.salesOrderHeader.shipMethod = 'FedEx';
        this.dateArrival.setDate(this.dateArrival.getDate()+10);
        break;
      case "3": //USPS - 12
        this.salesOrderHeader.shipMethod = 'USPS';
        this.dateArrival.setDate(this.dateArrival.getDate()+12);
        break;
      case "4": //DHL - 5
        this.salesOrderHeader.shipMethod = 'DHL';
        this.dateArrival.setDate(this.dateArrival.getDate()+5);
        break;
    }
    this.fillDetails();
    this.checkoutService.getOrderFreightCost(this.salesOrderHeader, this.salesOrderHeader.shipMethod).then(
      (data: any) => {
        this.salesOrderHeader.freight = data;
        this.GetTotalDue();
      }
    ).catch((err: Error) => {
      console.log(err.message);
    });
    this.GetTotalDue();
  }

  private CalculateTaxAmount(): number { //ok
    this.taxAmount = ((this.salesOrderHeader.subTotal * this.salesOrderHeader.taxAmt)/100);
    return this.taxAmount;
  }

  private GetTaxPercent(){ //ok
    var taxPercent = 0;
    var billingAddress = this.GetAddressById();
    switch(billingAddress?.countryRegion.toLowerCase()){
      case 'italia':
      case 'italy':
        this.salesOrderHeader.taxAmt = 23;
        break;
      case 'usa':
      case 'united states':
        this.salesOrderHeader.taxAmt = 26;
        break;
      case 'canada':
        this.salesOrderHeader.taxAmt = 15;
        break;
      case 'united kingdom':
      case 'regno unito':
        this.salesOrderHeader.taxAmt = 20;
        break;
    }
  }
  GetTotalDue(){ //ok
    if(this.salesOrderHeader.shipToAddressId != 0 && this.salesOrderHeader.billToAddressId != 0 && this.salesOrderHeader.shipMethod != ''){
      this.GetTaxPercent()
      var totalDue = 0;
      totalDue = this.salesOrderHeader.subTotal + this.CalculateTaxAmount() + this.salesOrderHeader.freight;
      this.salesOrderHeader.totalDue = totalDue;
    }
  }

  private GetAddressById(){
    var address = new Address();
    this.addresses.forEach(add =>{
      if(add.addressId == this.salesOrderHeader.billToAddressId)
        address = add;
    })
    return address;
  }

  fillDetails(){
    this.basketItemsProductsMap.forEach((prod, item) =>{
      var detail = {
        salesOrderid: 0,
        salesOrderDetailId: 0,
        orderQty: item.quantity,
        productId: prod.productId,
        unitPrice: prod.listPrice,
        unitPriceDiscount: 0,
        lineTotal: (prod.listPrice * item.quantity),
        modifiedDate: new Date(Date.now()),
        product: null
      } as SalesOrderDetail;
      this.salesOrderHeader.salesOrderDetails.push(detail)
    });
  }

  async sendOrder() {
    if(!this.isCheckoutDisabled){
      this.salesOrderHeader.orderDate = new Date(Date.now());
      var postResponse = await this.checkoutService.postSalesOrder(this.salesOrderHeader);
      try {
        await this.shoppingCartService.clearBasket();
      } catch (error) {
        console.log(error);
      }
      this.router.navigate(['/order-summary']);
    }
  }
}
