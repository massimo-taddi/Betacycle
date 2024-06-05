import { Component, OnInit } from '@angular/core';
import { Customer } from '../../../shared/models/Customer';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { jwtDecode } from 'jwt-decode';
import { Router, RouterModule } from '@angular/router';
import { SearchParams } from '../../../shared/models/SearchParams';
import { PasswordResetComponent } from '../../../core/password-reset/password-reset.component';
import { Panel, PanelModule } from 'primeng/panel';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { PrimeNGConfig } from 'primeng/api';

@Component({
  selector: 'app-personal-info',
  standalone: true,
  imports: [RouterModule, PasswordResetComponent, PanelModule, CommonModule, DialogModule, ButtonModule, ToastModule],
  templateUrl: './personal-info.component.html',
  styleUrl: './personal-info.component.css',
  providers: [MessageService]
})
export class PersonalInfoComponent implements OnInit {
  info?: Customer;
  changesNotAllowed: boolean = true;
  resetPwd: boolean = false;
  dialogBoolDelete: boolean = false;
  constructor(private httpInfo: HttpUserAdminService, private router: Router, private authService: AuthenticationService,
              private messageService: MessageService, private primengConfig: PrimeNGConfig) {}

  ngOnInit(): void {
    this.getPersonalInfo();
    this.primengConfig.ripple = true;
    }

  private getPersonalInfo() {
    this.httpInfo.httpGetCustomerInfo().subscribe({
      next: (infoList: any) => {
        this.info = infoList.item2.pop();
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }

  getJwtUsername():string {
    if(sessionStorage.getItem('jwtToken') == null) return 'user'
    var decodedToken: any = jwtDecode(sessionStorage.getItem('jwtToken')!);
    return decodedToken.unique_name;
  }

  DeleteAccount(){
    this.httpInfo.httpDeleteCustomer().subscribe({
      next: (data: any) =>{
        this.authService.setLoginStatus(false,'', false, false);
        this.showSuccess('Account deleted successfully');
        setTimeout(() => {
          this.router.navigate(["/home"]);
        }, 5000); 
      },
      error: (err: Error) =>{
        console.log(err);
      }
    })
  }

  showSuccess(content: string) {
    this.messageService.add({severity:'success', summary: 'Success', detail: content});
  }
}
