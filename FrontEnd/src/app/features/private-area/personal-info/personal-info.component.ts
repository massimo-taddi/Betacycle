import { Component, OnInit } from '@angular/core';
import { Customer } from '../../../shared/models/Customer';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';

@Component({
  selector: 'app-personal-info',
  standalone: true,
  imports: [],
  templateUrl: './personal-info.component.html',
  styleUrl: './personal-info.component.css'
})
export class PersonalInfoComponent implements OnInit {
  info?: Customer;

  constructor(private httpInfo: HttpUserAdminService) {}

  ngOnInit(): void {
    this.getPersonalInfo();
  }

  private getPersonalInfo() {
    this.httpInfo.httpGetCustomerInfo().subscribe({
      next: (infoList: Customer[]) => {
        this.info = infoList.pop();
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
}
