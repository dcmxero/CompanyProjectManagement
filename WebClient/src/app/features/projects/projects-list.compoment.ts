import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProjectsService, ProjectDto, CreateProjectDto } from '../../services/projects.service';

@Component({
  selector: 'app-projects-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <!-- FIXED toolbar aligned to card width -->
    <div class="fixed-toolbar">
      <div class="fixed-toolbar-inner">
        <button class="logout-fab" (click)="logout()" aria-label="Logout">Logout</button>
      </div>
    </div>

    <div class="page">
      <div class="card">
        <header class="bar">
          <h2>Projects</h2>
          <div class="actions">
            <button class="btn primary" (click)="openCreate()">+ New</button>
          </div>
        </header>

        <div *ngIf="!items.length" class="empty">No projects found.</div>

        <table class="grid" *ngIf="items.length">
          <thead>
            <tr>
              <th style="width:110px;">Id</th>
              <th>Name</th>
              <th style="width:150px;">Abbrev.</th>
              <th>Customer</th>
              <th style="width:120px;"></th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let p of items; trackBy: track">
              <td>
                <strong>{{ p.id }}</strong>
              </td>
              <td>{{ p.name }}</td>
              <td>{{ p.abbreviation }}</td>
              <td>{{ p.customer }}</td>
              <td class="right">
                <button class="btn sm" (click)="openEdit(p.id)">Edit</button>
                <button class="btn sm danger" (click)="remove(p.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>

        <p *ngIf="error" class="err">{{ error }}</p>
      </div>
    </div>

    <!-- BACKDROP -->
    <div class="backdrop" *ngIf="modalOpen" (click)="close()"></div>

    <!-- MODAL -->
    <div class="modal" *ngIf="modalOpen" role="dialog" aria-modal="true" (click)="close()">
      <div class="modal-card" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h3 class="modal-title">{{ isNew ? 'Create project' : 'Edit project' }}</h3>
          <button class="modal-close" type="button" (click)="close()" aria-label="Close">
            <svg width="12" height="12" viewBox="0 0 12 12" fill="none" aria-hidden="true">
              <path d="M3 3l6 6M9 3L3 9" stroke="#444" stroke-width="1.5" stroke-linecap="round" />
            </svg>
          </button>
        </div>

        <form class="modal-content" [formGroup]="form" (ngSubmit)="save()">
          <div class="row">
            <label>Name</label>
            <input class="input" formControlName="name" />
          </div>

          <div class="row">
            <label>Abbreviation</label>
            <input class="input" formControlName="abbreviation" />
          </div>

          <div class="row">
            <label>Customer</label>
            <input class="input" formControlName="customer" />
          </div>

          <p *ngIf="formError" class="err">{{ formError }}</p>

          <div class="modal-actions">
            <button class="btn primary" type="submit" [disabled]="form.invalid">
              {{ isNew ? 'Create' : 'Save' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [
    `
      :host {
        display: block;
      }

      /* === JEDNODUCHÝ FIXNÝ LOGOUT VPRAVO HORE VO VIEWPORTE === */
      .fixed-toolbar {
        position: fixed;
        top: 16px;
        right: 24px; /* pekné odsadenie od pravého okraja */
        z-index: 120; /* nad obsahom aj nad modalom */
      }
      .fixed-toolbar-inner {
        /* už netreba nič riešiť so šírkou karty */
        display: block;
      }
      .logout-fab {
        padding: 8px 20px;
        border-radius: 999px;
        border: 1px solid #d1d5db;
        background: #ffffff;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
        cursor: pointer;
        font-weight: 600;
        transition: transform 0.15s ease, box-shadow 0.15s ease, background 0.15s ease;
      }
      .logout-fab:hover {
        background: #f8fafc;
        transform: translateY(-1px);
        box-shadow: 0 8px 16px rgba(0, 0, 0, 0.12);
      }

      /* === PAGE / CARD === */
      .page {
        min-height: 100vh;
        display: flex;
        align-items: flex-start;
        justify-content: center;
        padding: 96px 16px 48px; /* horné odsadenie, aby nič nebolo "pod" Logoutom */
      }
      .card {
        width: min(1040px, 96vw);
        margin: 0 auto;
        padding: 24px;
        background: rgba(255, 255, 255, 0.9);
        border-radius: 16px;
        box-shadow: 0 12px 30px rgba(0, 0, 0, 0.1);
        backdrop-filter: blur(4px);
      }
      .bar {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 16px;
      }
      .bar h2 {
        margin: 0;
        font-size: 26px;
      }
      .actions {
        display: flex;
        gap: 10px;
      }

      /* === GRID === */
      .grid {
        width: 100%;
        border-collapse: collapse;
        text-align: center;
      }
      .grid th,
      .grid td {
        padding: 16px 14px;
        border-bottom: 1px solid #eef1f4;
      }
      .grid th:first-child,
      .grid td:first-child {
        text-align: left;
      }
      .grid tbody tr:hover {
        background: #fafbfd;
      }
      .empty {
        opacity: 0.7;
        padding: 18px 4px;
      }
      .err {
        color: crimson;
        margin-top: 10px;
      }
      .right {
        text-align: center;
      }

      /* === BUTTONS === */
      .btn {
        border: 1px solid #cbd5e1;
        background: #fff;
        padding: 8px 12px;
        border-radius: 10px;
        cursor: pointer;
      }
      .btn.sm {
        padding: 6px 10px;
        border-radius: 8px;
      }
      .btn.primary {
        background: #2d6cdf;
        color: #fff;
        border-color: #2d6cdf;
      }
      .btn.danger {
        color: #d9534f;
        border-color: #d9534f;
        background: #fff;
      }
      .btn.ghost {
        background: #f8fafc;
      }

      .input {
        width: 100%;
        padding: 11px 12px;
        border: 1px solid #d1d5db;
        border-radius: 10px;
      }

      /* === BACKDROP + MODAL === */
      .backdrop {
        position: fixed;
        inset: 0;
        background: rgba(0, 0, 0, 0.45);
        backdrop-filter: blur(3px);
        z-index: 40;
        animation: fadeIn 0.12s ease-in;
      }
      .modal {
        position: fixed;
        inset: 0;
        display: grid;
        place-items: center;
        padding: 24px;
        z-index: 50;
        animation: fadeIn 0.12s ease-in;
      }

      .modal-card {
        position: relative;
        max-width: 480px;
        width: calc(420px + 2 * 30px);
        background: #fff;
        border-radius: 16px;
        box-shadow: 0 24px 60px rgba(0, 0, 0, 0.28);
        padding: 16px 30px 18px;
        transform: translateY(-6px) scale(0.985);
        animation: popIn 0.16s ease-out forwards;
      }
      .modal-header {
        position: relative;
        padding: 6px 0 6px;
        border-bottom: 1px solid #eef0f3;
      }
      .modal-title {
        margin: 0 36px 10px;
        text-align: center;
        font-size: 18px;
        font-weight: 600;
      }
      .modal-close {
        position: absolute;
        top: 8px;
        right: 8px;
        width: 28px;
        height: 28px;
        background: transparent; /* bez rámčeka */
        border: none; /* bez rámčeka */
        padding: 0;
        display: grid;
        place-items: center;
        cursor: pointer;
      }
      .modal-close:hover {
        background: #e6ebf3;
      }
      .modal-content {
        display: grid;
        gap: 12px;
        width: 420px;
        margin: 14px auto 0;
      }
      .row {
        display: grid;
        gap: 6px;
      }
      .modal-actions {
        display: flex;
        justify-content: center;
        gap: 10px;
        margin-top: 8px;
        padding-top: 10px;
        border-top: 1px solid #eef0f3;
      }

      @media (max-width: 520px) {
        .page {
          padding-top: 112px;
        } /* trošku viac na mobile */
        .modal-card {
          max-width: 92vw;
          width: auto;
          padding-left: 20px;
          padding-right: 20px;
        }
        .modal-content {
          width: 86vw;
        }
        .modal-title {
          margin: 0 30px 10px;
        }
      }
    `,
  ],
})
export class ProjectsListComponent implements OnInit {
  items: ProjectDto[] = [];
  error = '';

  modalOpen = false;
  isNew = true;
  currentId: string | null = null;

  form!: FormGroup;
  formError = '';

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
    this.modalOpen = true;
    document.body.style.overflow = 'hidden';
  }

  openEdit(id: string): void {
    this.isNew = false;
    this.currentId = id;
    this.formError = '';
    this.api.getById(id).subscribe({
      next: (p) => {
        this.form.setValue({ name: p.name, abbreviation: p.abbreviation, customer: p.customer });
        this.modalOpen = true;
        document.body.style.overflow = 'hidden';
      },
      error: (e) => (this.error = e?.error?.error ?? 'Failed to load project'),
    });
  }

  close(): void {
    this.modalOpen = false;
    document.body.style.overflow = '';
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
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

  @HostListener('document:keydown.escape') onEsc() {
    if (this.modalOpen) this.close();
  }
}
