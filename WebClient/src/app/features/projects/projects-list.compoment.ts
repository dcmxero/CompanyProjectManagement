import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ProjectsService, ProjectDto } from '../../services/projects.service';

@Component({
  selector: 'app-projects-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="glass page">
      <header class="bar">
        <h2>Projects</h2>
        <div class="actions">
          <button class="primary" (click)="create()">+ New</button>
          <button class="ghost" (click)="logout()">Logout</button>
        </div>
      </header>

      <div *ngIf="!items.length" class="empty">No projects found.</div>

      <table class="grid" *ngIf="items.length">
        <thead>
          <tr>
            <th style="width:110px;">Id</th>
            <th>Name</th>
            <th style="width:160px;">Abbrev.</th>
            <th>Customer</th>
            <th style="width:110px;"></th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let p of items">
            <td>{{ p.id }}</td>
            <td>{{ p.name }}</td>
            <td>{{ p.abbreviation }}</td>
            <td>{{ p.customer }}</td>
            <td class="right">
              <button class="small" (click)="edit(p.id)">Edit</button>
              <button class="small danger" (click)="remove(p.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `,
  styles: [
    `
      .page {
        max-width: 1100px;
        margin: 40px auto;
        padding: 20px;
      }
      .bar {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 14px;
      }
      .actions {
        display: flex;
        gap: 8px;
      }
      .grid {
        width: 100%;
        border-collapse: collapse;
      }
      .grid th,
      .grid td {
        padding: 10px 12px;
        border-bottom: 1px solid #e6e6e6;
      }
      .grid tbody tr:hover {
        background: #fafafa;
      }
      .right {
        text-align: right;
      }
      .primary {
        background: #2d6cdf;
        color: #fff;
        border: 0;
        padding: 8px 12px;
        border-radius: 8px;
        cursor: pointer;
      }
      .ghost {
        background: transparent;
        border: 1px solid #aaa;
        padding: 8px 12px;
        border-radius: 8px;
        cursor: pointer;
      }
      .small {
        padding: 6px 10px;
        border: 1px solid #bbb;
        border-radius: 8px;
        background: #fff;
        cursor: pointer;
      }
      .danger {
        border-color: #d9534f;
        color: #d9534f;
      }
      .empty {
        opacity: 0.7;
        padding: 18px 4px;
      }
    `,
  ],
})
export class ProjectsListComponent implements OnInit {
  items: ProjectDto[] = [];
  constructor(private api: ProjectsService, private router: Router) {}
  ngOnInit() {
    this.refresh();
  }
  refresh() {
    this.api.getAll().subscribe((x) => (this.items = x));
  }
  edit(id: string) {
    this.router.navigate(['/projects', id]);
  }
  create() {
    this.router.navigate(['/projects/new']);
  }
  remove(id: string) {
    if (!confirm('Delete this project?')) return;
    this.api.delete(id).subscribe(() => this.refresh());
  }
  logout() {
    localStorage.removeItem('cpm.jwt');
    this.router.navigate(['/login']);
  }
}
