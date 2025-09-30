import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../Core/Services/auth-service';
import { ILogin } from '../../Core/Models/auth';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  constructor(private _auth: AuthService, private _router: Router) {}
  msg = '';
  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  });

  onSubmit() {
    this._auth.login(this.loginForm.value as ILogin).subscribe({
      next: (data) => {},
      error: (err) => {
        this.msg = err.message;
      },
    });
  }
  register() {
    this._router.navigate(['/register']);
  }
}
