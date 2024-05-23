import { Component, OnInit } from '@angular/core';
import { HttpUserAdminService } from '../../shared/services/http-user-admin.service';
import { Address } from '../../shared/models/Address';
import { CommonModule } from '@angular/common';
import { SalesOrderHeader } from '../../shared/models/SalesOrderHeader';
import { BasketService } from '../../shared/services/basket.service';
import { ShoppingCartItem } from '../../shared/models/ShoppingCartItem';
import { Product } from '../../shared/models/Product';
import { ProductService } from '../../shared/services/product.service';

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

  constructor(private http: HttpUserAdminService, private shoppingCartService: BasketService, private productService: ProductService){}

  ngOnInit(): void {
    this.http.httpGetCustomerAddresses().subscribe({
      next: (data: Address[]) =>{this.addresses = data},
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
    this.salesOrderHeader.shipToAddressID = this.addresses[radios[0].value as unknown as number].addressId;
  }
  setBillAddress(name: string){
    const radios = document.querySelectorAll<HTMLInputElement>(`input[name="${name}"]:checked`);
    this.salesOrderHeader.billToAddressID = this.addresses[radios[0].value as unknown as number].addressId;

  }

  // private GetSubTotal(): number { //ok
  //   console.log(this.basketItemsProductsMap)
  //   var totalPrice = 0;
  //   this.basketItemsProductsMap.forEach((product, item) => {
  //   totalPrice += (product.listPrice * item.quantity);
  //   });
  //   this.salesOrderHeader.subTotal = totalPrice;
  //   this.subTotalPrice = totalPrice;
  //   return totalPrice;
  // }

  private CalculateTaxAmount(): number { //ok
    this.taxAmount = ((this.salesOrderHeader.subTotal * this.salesOrderHeader.taxAmt)/100);
    return this.taxAmount;
  }

  private GetTaxPercent(){ //ok
    var taxPercent = 0;
    var billingAddress = this.GetAddressById();
    console.log(billingAddress?.countryRegion.toLowerCase())
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
    }
  }
  GetTotalDue(){ //ok
    if(this.salesOrderHeader.shipToAddressID != 0){
      this.GetTaxPercent()
      var totalDue = 0;
      totalDue = this.salesOrderHeader.subTotal + this.CalculateTaxAmount();
      this.salesOrderHeader.totalDue = totalDue;
    }
  }

  private GetAddressById(){
    var address = new Address();
    this.addresses.forEach(add =>{
      if(add.addressId == this.salesOrderHeader.billToAddressID)
        address = add;
    })
    return address;
  }
}
