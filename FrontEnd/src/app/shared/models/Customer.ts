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
  CustomerAddresses: CustomerAddress[] = [];

  //public virtual ICollection<SalesOrderHeader> SalesOrderHeaders ();

  constructor(
    Title: string,
    FirstName: string,
    MiddleName: string,
    LastName: string,
    Suffix: string,
    CompanyName: string,
    SalesPerson: string,
    EmailAddress: string,
    Phone: string,
    Password: string,
    ModifiedDate: Date,
    IsMigrated: boolean,
    CustomerAddresses: CustomerAddress[] = []
  ) {
    this.Title = Title;
    this.FirstName = FirstName;
    this.MiddleName = MiddleName;
    this.LastName = LastName;
    this.Suffix = Suffix;
    this.CompanyName = CompanyName;
    this.SalesPerson = SalesPerson;
    this.EmailAddress = EmailAddress;
    this.Phone = Phone;
    this.PasswordHash = Password;
    this.ModifiedDate = ModifiedDate;
    this.IsMigrated = IsMigrated;
    this.CustomerAddresses = CustomerAddresses;  
  }
}
