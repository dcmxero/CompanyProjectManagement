import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="glass page" style="max-width:460px; margin:64px auto; padding:24px;">
      <h2>Sign in</h2>
      <form [formGroup]="form" (ngSubmit)="submit()">
        <label>Username <input formControlName="username" /></label>
        <label>Password <input type="password" formControlName="password" /></label>
        <button type="submit" [disabled]="form.invalid || loading">Login</button>
        <p class="err" *ngIf="error">{{ error }}</p>
      </form>
    </div>
  `,
  styles: [
    `
      .page {
        max-width: 420px;
        margin: 64px auto;
        display: grid;
        gap: 12px;
      }
      label {
        display: grid;
        gap: 6px;
      }
      input {
        padding: 8px 10px;
        border: 1px solid #ccc;
        border-radius: 8px;
      }
      button {
        padding: 10px 14px;
        border: 0;
        border-radius: 10px;
        cursor: pointer;
      }
      .err {
        color: crimson;
      }
    `,
  ],
})
export class LoginComponent implements OnInit {
  loading = false;
  error = '';
  form!: FormGroup;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  submit() {
    if (this.form.invalid) {
      return;
    }
    const { username, password } = this.form.value as any;
    this.loading = true;
    this.auth.login(username, password).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/projects']);
      },
      error: (e) => {
        this.loading = false;
        console.error('LOGIN ERROR', e);
        this.error = e?.error?.error ?? 'Login failed';
      },
    });
  }
}
