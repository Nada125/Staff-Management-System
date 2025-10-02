import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../Core/Services/auth-service';

@Component({
  selector: 'app-verify-code',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './verify-code.html',
  styleUrls: ['./verify-code.css'],
})
export class VerifyCode implements OnInit {
  msg = '';
  purpose: 'EmailVerification' | 'PasswordReset' = 'EmailVerification';
  email = '';

  verifyCodeForm = new FormGroup({
    code: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(6),
      Validators.pattern('^[0-9]*$'),
    ]),
  });

  constructor(
    private _auth: AuthService,
    private _router: Router,
    private _route: ActivatedRoute
  ) {}

  ngOnInit() {
    this._route.queryParams.subscribe((params) => {
      this.purpose = params['purpose'] || 'EmailVerification';

      const storedEmail = localStorage.getItem('resetEmail');
      this.email = storedEmail || '';

      if (!this.email) {
        if (this.purpose === 'PasswordReset') {
          this._router.navigate(['/forgot-password']);
        } else {
          this._router.navigate(['/register']);
        }
      }
    });
  }

  onSubmit() {
    if (this.verifyCodeForm.invalid || !this.email) return;

    const code = this.verifyCodeForm.get('code')?.value!;
    this._auth.verifyCode(this.email, code, this.purpose).subscribe({
      next: (data) => {
        console.log('Code verified:', data);

        if (this.purpose === 'PasswordReset') {
          this._router.navigate(['/reset-password']);
        } else {
          this._router.navigate(['/login']);
        }
      },
      error: (err) => {
        this.msg = err.error?.message || 'Invalid verification code';
      },
    });
  }
}
