import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  inject,
  signal,
  computed,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { PeriodApiService } from '../../core/services/period-api.service';
import { HighamScoreService } from '../../core/services/higham-score.service';
import { CountChangeEvent, PeriodDetail } from '../../core/models/period.model';
import { ScoreGridComponent } from '../../shared/components/score-grid/score-grid.component';

@Component({
  selector: 'app-period-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, ScoreGridComponent],
  templateUrl: './period-detail.component.html',
  styleUrl: './period-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PeriodDetailComponent implements OnInit {
  @Input() id!: string;

  private readonly api = inject(PeriodApiService);
  private readonly router = inject(Router);
  readonly scoreService = inject(HighamScoreService);

  detail = signal<PeriodDetail | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  saveError = signal<string | null>(null);

  grandTotal = computed(() =>
    this.detail()?.rows.reduce((s, r) => s + r.rowTotal, 0) ?? 0
  );

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.api.getPeriod(Number(this.id)).subscribe({
      next: (data) => {
        this.detail.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load period.');
        this.loading.set(false);
      },
    });
  }

  onCountChanged(event: CountChangeEvent): void {
    // Optimistic local update
    this.detail.update((d) => {
      if (!d) return d;
      const rows = d.rows.map((row) => {
        if (row.criterionId !== event.criterionId) return row;
        const countsByDay = { ...row.countsByDay, [event.dayNumber]: event.count };
        const rowTotal =
          Object.values(countsByDay).reduce((s, c) => s + c, 0) *
          row.multiplier;
        return { ...row, countsByDay, rowTotal };
      });
      return { ...d, rows };
    });

    // Persist
    this.api
      .setCount(Number(this.id), event.dayNumber, event.criterionId, event.count)
      .subscribe({
        error: () => {
          this.saveError.set('Save failed. Please try again.');
          setTimeout(() => this.saveError.set(null), 3000);
        },
      });
  }

  addDay(): void {
    this.api.addDay(Number(this.id)).subscribe({
      next: (updated) => this.detail.set(updated),
      error: () => this.saveError.set('Failed to add day.'),
    });
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString(undefined, {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  back(): void {
    this.router.navigate(['/periods']);
  }
}
