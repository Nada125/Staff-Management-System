import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EmployeeService } from '../../../Core/Services/employee-service';
import { IEmployee } from '../../../Core/Models/Employee';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './employee-form.html',
  styleUrls: ['./employee-form.css'],
})
export class EmployeeForm implements OnInit {
  @Input() employee?: IEmployee;

  constructor(private _employee: EmployeeService) {}

  myForm: FormGroup = new FormGroup({
    userName: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    phoneNumber: new FormControl('', Validators.required),
    position: new FormControl('', Validators.required),
    nationality: new FormControl('', Validators.required),
    nationalId: new FormControl('', Validators.required),
    salary: new FormControl(0, Validators.required),
  });

  ngOnInit(): void {
    if (this.employee) {
      this.myForm.patchValue(this.employee);
    }
  }

  onSubmit() {
    if (this.myForm.invalid) return;

    const formValue: IEmployee = {
      id: this.employee?.id || '',
      ...this.myForm.value,
    };

    if (this.employee) {
      this._employee.editEmployee(formValue.id, formValue).subscribe({
        next: (res) => {
          console.log('Employee updated:', res);
        },
        error: (err) => {
          console.error('Error updating employee:', err);
        },
      });
    } else {
      this._employee.addEmployee(formValue).subscribe({
        next: (res) => {
          console.log('Employee added:', res);
          this.myForm.reset();
        },
        error: (err) => {
          console.error('Error adding employee:', err);
        },
      });
    }
  }
}
