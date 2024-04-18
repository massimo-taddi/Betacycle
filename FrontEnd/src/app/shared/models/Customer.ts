import { CustomerAddress } from './CustomerAddress';

export class Customer {
  CustomerId: number = 0;
  NameStyle: boolean = false;
  Title: string = '';
  FirstName: string = '';
  MiddleName: string = '';
  LastName: string = '';
  Suffix: string = '';
  CompanyName: string = '';
  SalesPerson: string = '';
  EmailAddress: string = '';
  Phone: string = '';
  PasswordHash: string = '';
  PasswordSalt: string = '';
  Rowguid: string = '';
  ModifiedDate: Date | null = null;
  IsMigrated: boolean = false;
  CustomerAddress: CustomerAddress[] = [];

  //public virtual ICollection<SalesOrderHeader> SalesOrderHeaders ();
}
