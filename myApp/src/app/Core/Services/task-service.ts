import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/env.dev';
import { ITask } from '../Models/Task';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private url = environment.apiURL + '/EmployeeTask';

  constructor(private _http: HttpClient) {}

  getTasks() {
    return this._http.get<ITask[]>(this.url);
  }

  getTaskById(id: number) {
    return this._http.get<ITask>(`${this.url}/${id}`);
  }

  addTask(task: ITask) {
    return this._http.post<ITask>(this.url, task);
  }

  editTask(id: number, task: ITask) {
    return this._http.patch<ITask>(`${this.url}/${id}`, task);
  }

  deleteTask(id: number) {
    return this._http.delete(`${this.url}/${id}`);
  }
}
