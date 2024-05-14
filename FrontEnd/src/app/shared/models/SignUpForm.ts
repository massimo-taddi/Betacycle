import { CustomerAddress } from "./CustomerAddress";

export class SignUpForm {
    title: string | null = null;
    firstName: string = '';
    middleName: string | null = null;
    lastName: string = '';
    suffix: string | null = null;
    companyName: string | null = null;
    salesPerson: string | null = null;
    emailAddress: string = '';
    phone: string | null = null;
    password: string = '';
    isMigrated: boolean = false;
    customerAddresses: CustomerAddress[] | null = null;
}