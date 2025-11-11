import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login.component').then((m) => m.LoginComponent),
  },
  { path: '', redirectTo: 'projects', pathMatch: 'full' },
  {
    path: 'projects',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/projects/projects-list.compoment').then((m) => m.ProjectsListComponent),
  },
  {
    path: 'projects/new',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/projects/project-edit.component').then((m) => m.ProjectEditComponent),
  },
  {
    path: 'projects/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/projects/project-edit.component').then((m) => m.ProjectEditComponent),
  },
  { path: '**', redirectTo: 'projects' },
];
