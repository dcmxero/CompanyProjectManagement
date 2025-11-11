import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ProjectsService, ProjectDto } from '../../services/projects.service';

@Component({
  selector: 'app-project-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="glass page" style="max-width:700px; margin:32px auto; padding:24px;">
      <h2>{{ isNew ? 'Create project' : 'Edit project' }}</h2>
      <form [formGroup]="form" (ngSubmit)="save()" style="display:grid;gap:12px;">
        <label>Name <input class="input" formControlName="name" /></label>
        <label>Abbreviation <input class="input" formControlName="abbreviation" /></label>
        <label>Customer <input class="input" formControlName="customer" /></label>

        <div style="display:flex;gap:8px;margin-top:4px;">
          <button class="btn" type="submit" [disabled]="form.invalid">Save</button>
          <button class="btn ghost" type="button" (click)="back()">Back</button>
          <button *ngIf="!isNew" class="btn danger" type="button" (click)="remove()">Delete</button>
        </div>
        <p class="err" *ngIf="error" style="color:crimson">{{ error }}</p>
      </form>
    </div>
  `,
  styles: [
    `
      .page {
        max-width: 700px;
        margin: 24px auto;
        padding: 0 12px;
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
      .actions {
        display: flex;
        gap: 8px;
        margin-top: 10px;
      }
      .ghost {
        background: transparent;
        border: 1px solid #aaa;
      }
      .danger {
        background: #d9534f;
        color: white;
      }
      button {
        padding: 8px 12px;
        border: 0;
        border-radius: 8px;
        cursor: pointer;
      }
      .err {
        color: crimson;
      }
    `,
  ],
})
export class ProjectEditComponent implements OnInit {
  isNew = true;
  error = '';
  id = '';
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private api: ProjectsService,
    private router: Router
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      name: ['', Validators.required],
      abbreviation: ['', Validators.required],
      customer: ['', Validators.required],
    });

    this.id = this.route.snapshot.paramMap.get('id') ?? '';
    this.isNew = !this.id || this.id === 'new';
    if (!this.isNew) {
      this.api.getById(this.id).subscribe((p) => this.form.patchValue(p));
    }
  }

  save() {
    const value = this.form.value as ProjectDto;
    if (this.isNew) {
      const { id, ...create } = value as any;
      this.api.create(create).subscribe({
        next: (p) => this.router.navigate(['/projects', p.id]),
        error: (e) => (this.error = e?.error?.error ?? 'Create failed'),
      });
    } else {
      value.id = this.id;
      this.api.update(value).subscribe({
        next: (_) => this.router.navigate(['/projects']),
        error: (e) => (this.error = e?.error?.error ?? 'Update failed'),
      });
    }
  }

  remove() {
    this.api.delete(this.id).subscribe({
      next: (_) => this.router.navigate(['/projects']),
      error: (e) => (this.error = e?.error?.error ?? 'Delete failed'),
    });
  }

  back() {
    this.router.navigate(['/projects']);
  }
}
