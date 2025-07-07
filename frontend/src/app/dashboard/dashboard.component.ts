import { Component, OnInit, ViewChild } from '@angular/core';
import { ChartConfiguration } from 'chart.js';
import { DashboardService } from '../dashboard.service';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      {
        label: 'Tickets',
        data: [],
        backgroundColor: ['#42A5F5', '#FFA726', '#66BB6A'],
      },
    ],
  };

  barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
  };

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getChartData().subscribe((data) => {
      console.log('API Data:', data);

      this.barChartData.labels = data.map(item => item.status);
      this.barChartData.datasets[0].data = data.map(item => item.count);

      this.chart?.update(); // ✅ Key line to make bars appear!
    });
  }
}
