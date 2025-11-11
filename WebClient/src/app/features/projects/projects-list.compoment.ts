import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProjectsService, ProjectDto, CreateProjectDto } from '../../services/projects.service';
import { ProjectModalComponent } from './project-modal.component';

@Component({
  selector: 'app-projects-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ProjectModalComponent],
  templateUrl: './projects-list.component.html',
  styleUrls: ['./projects-list.component.scss'],
})
export class ProjectsListComponent implements OnInit {
  items: ProjectDto[] = [];
  error = '';

  modalOpen = false;
  isNew = true;
  currentId: string | null = null;

  form!: FormGroup;
  formError = '';
  submitted = false;

  constructor(private api: ProjectsService, private fb: FormBuilder, private router: Router) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      abbreviation: ['', [Validators.required, Validators.maxLength(100)]],
      customer: ['', [Validators.required, Validators.maxLength(200)]],
    });
    this.refresh();
  }

  refresh(): void {
    this.api.getAll().subscribe({
      next: (x) => (this.items = x),
      error: (e) => (this.error = e?.error?.error ?? 'Failed to load projects'),
    });
  }

  track(_i: number, p: ProjectDto) {
    return p.id;
  }

  openCreate(): void {
    this.isNew = true;
    this.currentId = null;
    this.form.reset({ name: '', abbreviation: '', customer: '' });
    this.formError = '';
    this.submitted = false;
    this.modalOpen = true;
    document.body.style.overflow = 'hidden';
    setTimeout(() => this.focusFirstInvalid());
  }

  openEdit(id: string): void {
    this.isNew = false;
    this.currentId = id;
    this.formError = '';
    this.submitted = false;
    this.api.getById(id).subscribe({
      next: (p) => {
        this.form.setValue({
          name: p.name ?? '',
          abbreviation: p.abbreviation ?? '',
          customer: p.customer ?? '',
        });
        this.modalOpen = true;
        document.body.style.overflow = 'hidden';
        setTimeout(() => this.focusFirstInvalid());
      },
      error: (e) => (this.error = e?.error?.error ?? 'Failed to load project'),
    });
  }

  close(): void {
    this.modalOpen = false;
    document.body.style.overflow = '';
  }

  save(): void {
    this.submitted = true;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.focusFirstInvalid();
      return;
    }

    if (this.isNew) {
      const dto: CreateProjectDto = this.form.value as CreateProjectDto;
      this.api.create(dto).subscribe({
        next: () => {
          this.close();
          this.refresh();
        },
        error: (e) => (this.formError = e?.error?.error ?? 'Create failed'),
      });
    } else {
      const dto: ProjectDto = {
        id: this.currentId!,
        ...(this.form.value as Omit<ProjectDto, 'id'>),
      };
      this.api.update(dto).subscribe({
        next: () => {
          this.close();
          this.refresh();
        },
        error: (e) => (this.formError = e?.error?.error ?? 'Save failed'),
      });
    }
  }

  remove(id: string): void {
    if (!confirm('Delete this project?')) return;
    this.api.delete(id).subscribe({
      next: () => this.refresh(),
      error: (e) => (this.error = e?.error?.error ?? 'Delete failed'),
    });
  }

  logout(): void {
    localStorage.removeItem('cpm.jwt');
    this.router.navigate(['/login']);
  }

  private focusFirstInvalid() {
    const first = Object.keys(this.form.controls).find((k) => this.form.get(k)?.invalid);
    const el = first
      ? (document.querySelector(`[formControlName="${first}"]`) as HTMLElement)
      : null;
    el?.focus();
  }

  @HostListener('document:keydown.escape') onEsc() {
    if (this.modalOpen) this.close();
  }
}
