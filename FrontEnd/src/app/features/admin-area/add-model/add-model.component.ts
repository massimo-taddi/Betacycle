import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ProductModel } from '../../../shared/models/ProductModel';
import { Router } from '@angular/router';
import { ModelService } from '../../../shared/services/model.service';
import { InputTextareaModule } from 'primeng/inputtextarea';
@Component({
  selector: 'app-add-model',
  standalone: true,
  imports: [
    ButtonModule,
    InputSwitchModule,
    FormsModule,
    CardModule,
    FormsModule,
    InputTextareaModule,
  ],
  templateUrl: './add-model.component.html',
  styleUrl: './add-model.component.css',
})
export class AddModelComponent {
  model: ProductModel = new ProductModel();
  description: string = '';
  constructor(private router: Router, private service: ModelService) {}

  Post() {
    this.service
      .postModel(this.model.name, this.model.discontinued, this.description)
      .subscribe({
        next: (el: any) => {
          console.log('Success!');
        },
        error: (err: Error) => {
          console.log(err);
        },
      });
    if (this.description != '') {
      this.service.postModelDescription();
    }
    this.router.navigate(['/admin/model']);
  }
}
