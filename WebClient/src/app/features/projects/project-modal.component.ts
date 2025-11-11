import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-project-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './project-modal.component.html',
  styleUrls: ['./project-modal.component.scss'],
})
export class ProjectModalComponent {
  @Input() form!: FormGroup;
  @Input() isNew = true;
  @Input() submitted = false;
  @Input() formError = '';

  @Output() save = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();
  @Output() remove = new EventEmitter<void>();

  error(ctrl: string): boolean {
    const c = this.form.get(ctrl);
    return !!c && c.invalid && (c.touched || this.submitted);
  }

  onSubmit(): void {
    this.save.emit();
  }

  onClose(): void {
    this.close.emit();
  }

  onRemove(): void {
    this.remove.emit();
  }
}
