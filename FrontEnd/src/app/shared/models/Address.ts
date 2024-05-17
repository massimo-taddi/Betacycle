import { CustomerAddress } from './CustomerAddress';

export class Address {
  addressId: number = 0;
  addressLine1: string = '';
  addressLine2?: string = '';
  city: string = '';
  stateProvince: string = '';
  countryRegion: string = '';
  postalCode: string = '';
  rowguid: string = '';
  modifiedDate: Date;
  customerAddresses: CustomerAddress[] = [];

  constructor() {
    this.modifiedDate = new Date(Date.now());
  }
}
