import { CanMatchFn, Router } from '@angular/router';
import { AuthService } from '../Services/auth-service';
import { inject } from '@angular/core';

export const managerGuard: CanMatchFn = (route, segments) => {
  const _auth = inject(AuthService);
  const _router = inject(Router);

  if (_auth.isUserLoggedin() && _auth.isLoggedin()?.role === 'Manager') {
    return true;
  } else {
    // we fetch this url => http://localhost:4200/login?returnUrl=dashboard%2Fusers
    return _router.createUrlTree(['/login'], {
      queryParams: { returnUrl: segments.join('/') },
    });
  }
};
