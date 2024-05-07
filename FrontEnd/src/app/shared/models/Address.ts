export class Address {
  addressId: number = 0;
  addressLine1: string = '';
  addressLine2: string = '';
  city: string = '';
  stateProvince: string = '';
  countryRegion: string = '';
  postalCode: string = '';
  rowguid: string = '';
  modifiedDate: Date | null = null;

  constructor(){
    this.modifiedDate = new Date(Date.now());
  }
}
