import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EmployeeService } from '../../Core/Services/employee-service';
import { IEmployee } from '../../Core/Models/Employee';
import { CommonModule, CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-employee-dash',
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './employees.html',
  styleUrls: ['./employees.css'],
})
export class Employee implements OnInit {
  employees: IEmployee[] = [];

  constructor(private _employee: EmployeeService, private router: Router) {}

  ngOnInit() {
    this.getEmployees();
  }

  getEmployees() {
    this._employee.getEmployees().subscribe((data) => {
      this.employees = data;
    });
  }

  goToAdd() {
    this.router.navigate(['/dashboard/employee/add']);
  }

  goToEdit(id: string) {
    this.router.navigate(['/dashboard/employee/edit/', id]);
  }
}
