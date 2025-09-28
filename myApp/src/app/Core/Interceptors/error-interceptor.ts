import { HttpInterceptorFn } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../Services/auth-service';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const _router = inject(Router);
  const _auth = inject(AuthService);
  return next(req).pipe(
    catchError((error) => {
      if (error.status === 401) {
        _auth.logout();
        _router.navigate(['/login']);
      } else if (error.status === 403) {
        _router.navigate(['/unAuthorized']);
      } else if (error.status === 404) {
        _router.navigate(['/notFound']);
      } else {
        alert(error.error?.message || 'something went wrong, please try again');
      }
      return throwError(() => error);
    })
  );
};
