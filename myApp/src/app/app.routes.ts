import { Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { Employee } from './dashboard/employees/employees';
import { EmployeeForm } from './dashboard/employees/employee-form/employee-form';
import { Task } from './dashboard/tasks/tasks';
import { TaskForm } from './dashboard/tasks/task-form/task-form';
import { Report } from './dashboard/reports/reports';
import { ReportForm } from './dashboard/reports/report-form/report-form';
import { Auth } from './auth/auth';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { managerGuard } from './Core/Guards/manager-guard';
import { VerifyCode } from './auth/verify-code/verify-code';
import { ForgotPassword } from './auth/forgot-password/forgot-password';
import { ResetPassword } from './auth/reset-password/reset-password';

export const routes: Routes = [
  {
    path: '',
    component: Auth,
    children: [
      { path: 'login', component: Login },
      { path: 'register', component: Register },
      { path: 'verify-code', component: VerifyCode },
      { path: 'forgot-password', component: ForgotPassword },
      { path: 'reset-password', component: ResetPassword },
    ],
  },
  {
    path: 'dashboard',
    canMatch: [managerGuard],
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
