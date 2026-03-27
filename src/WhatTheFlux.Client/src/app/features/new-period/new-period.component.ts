import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PeriodApiService } from '../../core/services/period-api.service';

@Component({
  selector: 'app-new-period',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './new-period.component.html',
  styleUrl: './new-period.component.scss',
})
export class NewPeriodComponent {
  private readonly api = inject(PeriodApiService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  submitting = signal(false);
  error = signal<string | null>(null);

  form = this.fb.group({
    startDate: [this.todayIso(), Validators.required],
    notes: [''],
    initialDays: [7, [Validators.required, Validators.min(1), Validators.max(30)]],
  });

  private todayIso(): string {
    return new Date().toISOString().split('T')[0];
  }

  submit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);
    this.error.set(null);

    const v = this.form.value;
    this.api
      .createPeriod({
        startDate: v.startDate!,
        notes: v.notes || null,
        initialDays: v.initialDays ?? 7,
      })
      .subscribe({
        next: (period) => this.router.navigate(['/periods', period.id]),
        error: () => {
          this.error.set('Failed to create period.');
          this.submitting.set(false);
        },
      });
  }

  cancel(): void {
    this.router.navigate(['/periods']);
  }
}
