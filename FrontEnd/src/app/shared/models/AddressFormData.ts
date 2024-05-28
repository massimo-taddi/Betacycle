import { Address } from "./Address";
import { CustomerAddress } from "./CustomerAddress";

export class AddressFormData {

   addressLine1: string;
   addressLine2: string;
   city: string;
   stateProvince: string;
   countryRegion: string;
   postalCode: string;
   addressType: string;
   modifiedDate: Date = new Date(Date.now());
   isDeleted: boolean = false;
  
   constructor (
    addressLine1: string,
    addressLine2: string = '',
    city: string,
    stateProvince: string,
    countryRegion: string,
    postalCode: string,
    addressType: string,
    isDeleted: boolean
   ) {
    this.addressLine1 = addressLine1;
    this.addressLine2 = addressLine2;
    this.city = city;
    this.stateProvince = stateProvince;
    this.countryRegion = countryRegion;
    this.postalCode = postalCode;
    this.addressType = addressType;
    this.isDeleted = isDeleted;
   }

}