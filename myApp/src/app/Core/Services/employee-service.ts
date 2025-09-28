import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IEmployee } from '../Models/Employee';
import { environment } from '../../../environments/env.dev';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private url = `${environment.apiURL}/Employee`;

  constructor(private _http: HttpClient) {}

  getEmployees() {
    return this._http.get<IEmployee[]>(this.url);
  }

  getEmployeeById(id: string) {
    return this._http.get<IEmployee>(`${this.url}/${id}`);
  }
  getEmployeeByTasks(id: string) {
    return this._http.get<IEmployee>(`${this.url}/Task/${id}`);
  }
  addEmployee(employee: IEmployee) {
    return this._http.post<IEmployee>(this.url, employee);
  }

  editEmployee(id: string, employee: IEmployee) {
    return this._http.patch<IEmployee>(`${this.url}/${id}`, employee);
  }
}
