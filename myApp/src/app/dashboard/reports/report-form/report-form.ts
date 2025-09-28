import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IReport } from '../../../Core/Models/Report';
import { ReportService } from '../../../Core/Services/report-service';

@Component({
  selector: 'app-report-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './report-form.html',
  styleUrls: ['./report-form.css'],
})
export class ReportForm implements OnInit {
  @Input() report?: IReport;

  constructor(private _report: ReportService) {}

  myForm: FormGroup = new FormGroup({
    content: new FormControl('', Validators.required),
    submittedAt: new FormControl(new Date().toISOString(), Validators.required),
    employeeId: new FormControl('', Validators.required),
  });

  ngOnInit(): void {
    if (this.report) {
      this.myForm.patchValue(this.report);
    }
  }

  onSubmit() {
    if (this.myForm.invalid) return;

    const formValue: IReport = {
      id: this.report?.id || 0,
      ...this.myForm.value,
    };

    if (this.report) {
      this._report.editReport(formValue.id, formValue).subscribe({
        next: (res) => {
          console.log('Report updated:', res);
        },
        error: (err) => {
          console.error('Error updating report:', err);
        },
      });
    } else {
      this._report.addReport(formValue).subscribe({
        next: (res) => {
          console.log('Report added:', res);
          this.myForm.reset({
            submittedAt: new Date().toISOString(),
          });
        },
        error: (err) => {
          console.error('Error adding report:', err);
        },
      });
    }
  }
}
