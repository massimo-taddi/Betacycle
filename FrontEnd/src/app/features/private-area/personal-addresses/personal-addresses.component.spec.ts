import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonalAddressesComponent } from './personal-addresses.component';

describe('PersonalAddressesComponent', () => {
  let component: PersonalAddressesComponent;
  let fixture: ComponentFixture<PersonalAddressesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PersonalAddressesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PersonalAddressesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
