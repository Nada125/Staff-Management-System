import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute, Router, Routes } from '@angular/router';
import { BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../../environments/env.dev';
import { ILogin, ILoginRes, IRegister, IUserData } from '../Models/auth';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private _http: HttpClient,
    private _router: Router,
    private _activatedRoute: ActivatedRoute
  ) {}

  private isAuth = new BehaviorSubject<IUserData | null>(null);
  public isAuth$ = this.isAuth.asObservable();
  private url = environment.apiURL + '/Auth';
  private token_key = 'token';

  register(user: IRegister) {
    return this._http.post<IRegister>(`${this.url}/register`, user);
  }

  login(data: ILogin) {
    return this._http.post<ILoginRes>(this.url + '/login', data).pipe(
      tap((res) => {
        const token = res.token;
        if (token) {
          this.storeToken(token);
          const decode = this.decodeToken(token);
          this.isAuth.next(decode);
          if (decode?.role) {
            console.log(decode.role);
            //loginRouting
            if (decode.role === 'Manager') {
              this._router.navigate(['/dashboard']);
            } else {
              this._router.navigate(['/home']);
            }
          }
        }
      })
    );
  }

  // Verify code
  verifyCode(Email: string, Code: string, Purpose: 'EmailVerification' | 'PasswordReset') {
  const purposeMap: Record<'EmailVerification' | 'PasswordReset', number> = {
    EmailVerification: 0,
    PasswordReset: 1,
  };

  return this._http.post(`${this.url}/verify-code`, {
    Email,
    Code,
    Purpose: purposeMap[Purpose],
  });
}

  // Request password reset code
  forgotPassword(email: string) {
    return this._http.post(`${this.url}/forgot-password`, { email });
  }

  // Reset password with verification code
  resetPassword(email: string, code: string, newPassword: string, confirmPassword: string) {
    return this._http.post(`${this.url}/reset-password`, {
      email,
      code,
      newPassword,
      confirmPassword,
    });
  }

  loginRouting(role: string) {
    const query = this._activatedRoute.snapshot.queryParams;
    if (query) {
      const isManageRoute = query['returnurl']?.startsWith('/dashboard');
      const isEmployeeRoute = query['returnurl']?.startsWith('/home');
      if (role === 'Manager' && isManageRoute) {
        this._router.navigate([query['returnurl']]);
      }
      if (role === 'Employee' && isEmployeeRoute) {
        this._router.navigate([query['returnurl']]);
      }
    } else {
      if (role === 'Manager') {
        this._router.navigate(['/dashboard']);
      } else {
        this._router.navigate(['/home']);
      }
    }
  }

  isLoggedin(): IUserData | null {
    const token = this.getToken();
    if (token) {
      const decode = this.decodeToken(token);
      return decode;
    }
    return null;
  }

  ininitLogin() {
    this.isAuth.next(this.isLoggedin());
  }

  isUserLoggedin(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const decoded = this.decodeToken(token);
    return decoded !== null;
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    const decoded = this.decodeToken(token);
    return decoded === null;
  }

  logout() {
    localStorage.removeItem(this.token_key);
    this.isAuth.next(null);
    const routes = this._router.config;
    const currentURL = this._router.url;
    const isSecure = this.isSecureRoute(currentURL, routes);
    console.log('url', currentURL);
    console.log('routes', routes);

    if (isSecure === true) {
      this._router.navigate(['/Auth/login']);
    }
  }

  private isSecureRoute(url: string, routes: Routes): boolean {
    for (const myRoute of routes) {
      if (url.startsWith('/' + myRoute.path)) {
        if (myRoute.canActivate?.length) {
          return true;
        }
        if (myRoute.children) {
          return this.isSecureRoute(url, myRoute.children);
        }
      }
    }
    return false;
  }

  private decodeToken(token: string): IUserData | null {
    try {
      const decode = jwtDecode<IUserData>(token);
      if (!decode) {
        return null;
      }
      if (decode.exp) {
        const expiry = decode.exp * 1000;
        if (expiry > Date.now()) {
          return decode;
        }
      }
      return null;
    } catch (err) {
      console.error('Error decoding token:', err);
      return null;
    }
  }

  private storeToken(token: string) {
    if (localStorage) {
      localStorage.setItem(this.token_key, token);
    }
  }

  getToken() {
    return localStorage.getItem(this.token_key);
  }

  isManager(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const decoded = this.decodeToken(token);
    if (!decoded) return false;

    return decoded.role === 'Manager';
  }
}
