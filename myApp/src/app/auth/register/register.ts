import { Component } from '@angular/core';
import { IRegister } from '../../Core/Models/auth';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../Core/Services/auth-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  constructor(private _auth: AuthService) {}

  msg = '';

  registerForm = new FormGroup({
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
    role: new FormControl('Employee'), // default role
  });

  onSubmit() {
    this._auth.register(this.registerForm.value as IRegister).subscribe({
      next: (data) => {
        console.log('User registered:', data);
        this.registerForm.reset();
      },
      error: (err) => {
        this.msg = err.error?.message || 'Registration failed';
      },
    });
  }
}
