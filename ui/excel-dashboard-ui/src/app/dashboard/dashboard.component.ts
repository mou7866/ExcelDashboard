import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ApiService, DashboardRowDto } from '../api.service';
import { Chart, ChartConfiguration, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly api = inject(ApiService);

  rows = signal<DashboardRowDto[]>([]);
  totalCount = signal(0);
  pageSize = 20;
  pageIndex = signal(0);
  loading = signal(false);
  error = signal<string | null>(null);

  summaryLoading = signal(false);
  amountByCategory = signal<{ [category: string]: number }>({});

  private categoryChart?: Chart;

  ngOnInit(): void {
    this.loadPage(0);
    this.loadSummary();
  }

  ngOnDestroy(): void {
    this.categoryChart?.destroy();
  }

  loadPage(page: number): void {
    this.loading.set(true);
    this.error.set(null);
    const skip = page * this.pageSize;

    this.api.getRows(skip, this.pageSize).subscribe({
      next: res => {
        this.loading.set(false);
        this.pageIndex.set(page);
        this.rows.set(res.items);
        this.totalCount.set(res.totalCount);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Failed to load rows.');
      },
    });
  }

  loadSummary(): void {
    this.summaryLoading.set(true);
    this.api.getSummary().subscribe({
      next: res => {
        this.summaryLoading.set(false);
        this.amountByCategory.set(res.amountByCategory);
        this.renderChart();
      },
      error: () => {
        this.summaryLoading.set(false);
        this.error.set('Failed to load summary.');
      },
    });
  }

  nextPage(): void {
    const maxPage = Math.ceil(this.totalCount() / this.pageSize) - 1;
    if (this.pageIndex() < maxPage) {
      this.loadPage(this.pageIndex() + 1);
    }
  }

  prevPage(): void {
    if (this.pageIndex() > 0) {
      this.loadPage(this.pageIndex() - 1);
    }
  }

  private renderChart(): void {
    const canvas = document.getElementById('categoryChart') as HTMLCanvasElement | null;
    if (!canvas) {
      return;
    }

    const data = this.amountByCategory();
    const labels = Object.keys(data);
    const values = labels.map(k => data[k]);

    const config: ChartConfiguration<'bar'> = {
      type: 'bar',
      data: {
        labels,
        datasets: [
          {
            label: 'Amount by Category',
            data: values,
            backgroundColor: '#60a5fa',
          },
        ],
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            display: true,
          },
        },
        scales: {
          x: {
            ticks: {
              autoSkip: false,
            },
          },
        },
      },
    };

    this.categoryChart?.destroy();
    this.categoryChart = new Chart(canvas, config);
  }
}
