import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { PeriodApiService } from '../../core/services/period-api.service';
import { HighamScoreService } from '../../core/services/higham-score.service';
import { PeriodSummary } from '../../core/models/period.model';

@Component({
  selector: 'app-period-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './period-list.component.html',
  styleUrl: './period-list.component.scss',
})
export class PeriodListComponent implements OnInit {
  private readonly api = inject(PeriodApiService);
  private readonly router = inject(Router);
  readonly scoreService = inject(HighamScoreService);

  periods = signal<PeriodSummary[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.api.getPeriods().subscribe({
      next: (data) => {
        this.periods.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load periods.');
        this.loading.set(false);
      },
    });
  }

  viewPeriod(id: number): void {
    this.router.navigate(['/periods', id]);
  }

  newPeriod(): void {
    this.router.navigate(['/periods', 'new']);
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString(undefined, {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  delete(event: MouseEvent, id: number): void {
    event.stopPropagation();
    if (!confirm('Delete this period record?')) return;
    this.api.deletePeriod(id).subscribe(() => this.load());
  }
}
