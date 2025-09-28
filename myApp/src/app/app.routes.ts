import { Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { Employee } from './dashboard/employees/employees';
import { EmployeeForm } from './dashboard/employees/employee-form/employee-form';
import { Task } from './dashboard/tasks/tasks';
import { TaskForm } from './dashboard/tasks/task-form/task-form';
import { Report } from './dashboard/reports/reports';
import { ReportForm } from './dashboard/reports/report-form/report-form';

export const routes: Routes = [
  {
    path: 'dashboard',
    component: Dashboard,
    children: [
      {
        path: 'employee',
        children: [
          { path: '', component: Employee },
          { path: 'add', component: EmployeeForm },
          { path: 'edit/:id', component: EmployeeForm },
        ],
      },
      {
        path: 'task',
        children: [
          { path: '', component: Task },
          { path: 'add', component: TaskForm },
          { path: 'edit/:id', component: TaskForm },
        ],
      },
      {
        path: 'report',
        children: [
          { path: '', component: Report },
          { path: 'add', component: ReportForm },
          { path: 'edit/:id', component: ReportForm },
        ],
      },
    ],
  },
];
