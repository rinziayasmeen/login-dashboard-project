import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  onSubmit(): void {
  if (this.loginForm.invalid) return;

  const credentials = this.loginForm.value;

  this.auth.login(credentials).subscribe({
    next: () => {
      this.errorMessage = ''; // clear previous error
      this.router.navigate(['/dashboard']);
    },
    error: (err) => {
      if (err.status === 429) {
        this.errorMessage = 'Too many login attempts. Please try again later.';
      } else if (err.status === 401) {
        this.errorMessage = 'Invalid username or password.';
      } else {
        this.errorMessage = 'Something went wrong. Please try again.';
      }
    },
  });
}

}
