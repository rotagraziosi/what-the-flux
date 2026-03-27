import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { CountChangeEvent, PeriodDetail } from '../../../core/models/period.model';

@Component({
  selector: 'wtf-score-grid',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './score-grid.component.html',
  styleUrl: './score-grid.component.scss',
})
export class ScoreGridComponent implements OnInit, OnDestroy {
  @Input({ required: true }) detail!: PeriodDetail;
  @Output() countChanged = new EventEmitter<CountChangeEvent>();

  private readonly changes$ = new Subject<CountChangeEvent>();
  private readonly subscription = this.changes$
    .pipe(debounceTime(500))
    .subscribe((e) => this.countChanged.emit(e));

  get grandTotal(): number {
    return this.detail.rows.reduce((sum, r) => sum + r.rowTotal, 0);
  }

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  getCount(row: PeriodDetail['rows'][0], day: number): number {
    return row.countsByDay[day] ?? 0;
  }

  onInput(event: Event, dayNumber: number, criterionId: number): void {
    const input = event.target as HTMLInputElement;
    const count = Math.max(0, parseInt(input.value) || 0);
    input.value = String(count);
    this.changes$.next({ dayNumber, criterionId, count });
  }
}
