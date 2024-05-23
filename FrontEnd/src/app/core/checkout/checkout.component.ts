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
  totalPrice: number = 0;

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

  GetSubTotal() { 
    this.basketItemsProductsMap.forEach((product, item) => {
      this.totalPrice += (product.listPrice * item.quantity);
      console.log(this.totalPrice)
    });
    this.salesOrderHeader.subTotal = this.totalPrice;
  }

  CalculateTaxAmount(): number {
    return ((this.GetTaxPercent()/this.totalPrice)*100);
  }

  GetTaxPercent(): number{
    var taxPercent = 0;
    var billingAddress = this.addresses.find(add => add.addressId == this.salesOrderHeader.billToAddressID);
    switch(billingAddress?.countryRegion.toLowerCase()){
      case 'italy' || 'italia':
        return 23;
      case 'usa' || 'united states':
        return 26;
      case 'canada':
        return 15;
    }
    return taxPercent;
  }
  GetTotalDue(): number{
    return this.totalPrice + this.CalculateTaxAmount();
  }
}
