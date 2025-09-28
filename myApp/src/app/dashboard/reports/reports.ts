import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReportService } from '../../Core/Services/report-service';
import { IReport } from '../../Core/Models/Report';

@Component({
  selector: 'app-report',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.html',
  styleUrls: ['./reports.css'],
})
export class Report implements OnInit {
  reports: IReport[] = [];

  constructor(private _report: ReportService, private router: Router) {}

  ngOnInit() {
    this.getReports();
  }

  getReports() {
    this._report.getReports().subscribe((data) => {
      this.reports = data;
    });
  }

  goToAdd() {
    this.router.navigate(['/dashboard/report/add']);
  }

  goToEdit(id: number) {
    this.router.navigate(['/dashboard/report/edit/', id]);
  }
}
