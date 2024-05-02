import { CustomerAddress } from './CustomerAddress';

export class Customer {
  customerId: number = 0;
  nameStyle: boolean = false;
  title: string = '';
  firstName: string = '';
  middleName: string = '';
  lastName: string = '';
  suffix: string = '';
  companyName: string = '';
  salesPerson: string = '';
  emailAddress: string = '';
  phone: string = '';
  passwordHash: string = '';
  passwordSalt: string = '';
  rowguid: string = '';
  modifiedDate: Date | null = null;
  isMigrated: boolean = false;
  customerAddresses: CustomerAddress[] = [];

  //public virtual ICollection<SalesOrderHeader> SalesOrderHeaders ();

  constructor(
    title: string,
    firstName: string,
    middleName: string,
    lastName: string,
    suffix: string,
    companyName: string,
    salesPerson: string,
    emailAddress: string,
    phone: string,
    password: string,
    modifiedDate: Date,
    isMigrated: boolean,
    customerAddresses: CustomerAddress[] = []
  ) {
    this.title = title;
    this.firstName = firstName;
    this.middleName = middleName;
    this.lastName = lastName;
    this.suffix = suffix;
    this.companyName = companyName;
    this.salesPerson = salesPerson;
    this.emailAddress = emailAddress;
    this.phone = phone;
    this.passwordHash = password;
    this.modifiedDate = modifiedDate;
    this.isMigrated = isMigrated;
    this.customerAddresses = customerAddresses;  
  }
}
