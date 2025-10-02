import { Component } from '@angular/core';
import { IRegister } from '../../Core/Models/auth';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';
import { AuthService } from '../../Core/Services/auth-service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

// ✅ Validator للتأكد إن confirmPassword = password
export const passwordMatchValidator: ValidatorFn = (
  control: AbstractControl
): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  return password && confirmPassword && password.value !== confirmPassword.value
    ? { passwordMismatch: true }
    : null;
};

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  constructor(private _auth: AuthService, private _router: Router) {}

  msg = '';

  registerForm = new FormGroup(
    {
      userName: new FormControl('', [Validators.required, Validators.minLength(5)]),
      phoneNumber: new FormControl('', [
        Validators.required,
        Validators.minLength(5),
        Validators.maxLength(15),
        Validators.pattern('^[0-9]*$'),
      ]),
      email: new FormControl('', [Validators.required, Validators.email]),
      nationality: new FormControl(''),
      nationalId: new FormControl('', [Validators.required, Validators.pattern(/^[0-9]{14}$/)]),
      password: new FormControl('', [Validators.required, Validators.minLength(8)]),
      confirmPassword: new FormControl('', [Validators.required]),
      role: new FormControl('Employee'),
    },
    { validators: passwordMatchValidator }
  );

  onSubmit() {
    if (this.registerForm.invalid) return;

    const payload = this.registerForm.value as IRegister;

    this._auth.register(payload).subscribe({
      next: (data) => {
        console.log('User registered:', data);

        localStorage.setItem('resetEmail', payload.email);

        this._router.navigate(['/verify-code'], {
          queryParams: { purpose: 'EmailVerification' },
        });
      },
      error: (err) => {
        this.msg = err.error?.message || 'Registration failed';
      },
    });
  }
}
