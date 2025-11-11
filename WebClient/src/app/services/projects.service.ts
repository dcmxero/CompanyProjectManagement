import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environments';

export interface ProjectDto {
  id: string;
  name: string;
  abbreviation: string;
  customer: string;
}
export interface CreateProjectDto {
  name: string;
  abbreviation: string;
  customer: string;
}

@Injectable({ providedIn: 'root' })
export class ProjectsService {
  private base = `${environment.apiBaseUrl}/projects`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<ProjectDto[]> {
    return this.http.get<ProjectDto[]>(this.base);
  }
  getById(id: string): Observable<ProjectDto> {
    return this.http.get<ProjectDto>(`${this.base}/${id}`);
  }
  create(dto: CreateProjectDto): Observable<ProjectDto> {
    return this.http.post<ProjectDto>(this.base, dto);
  }
  update(id: string, dto: ProjectDto): Observable<ProjectDto> {
    return this.http.put<ProjectDto>(`${this.base}/${id}`, dto);
  }
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
