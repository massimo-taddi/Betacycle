import { Component, OnInit } from '@angular/core';
import { Customer } from '../../../shared/models/Customer';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { jwtDecode } from 'jwt-decode';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-personal-info',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './personal-info.component.html',
  styleUrl: './personal-info.component.css',
})
export class PersonalInfoComponent implements OnInit {
  info?: Customer;
  changesNotAllowed: boolean = true;

  constructor(private httpInfo: HttpUserAdminService, private router: Router) {}

  ngOnInit(): void {
    this.getPersonalInfo();
  }

  private getPersonalInfo() {
    /*
    this.httpInfo.httpGetCustomerInfo().subscribe({
      next: (infoList: Customer[]) => {
        this.info = infoList.pop();
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
    */
  }

  getJwtUsername(): string {
    if (sessionStorage.getItem('jwtToken') == null) return 'user';
    var decodedToken: any = jwtDecode(sessionStorage.getItem('jwtToken')!);
    return decodedToken.unique_name;
  }
}
