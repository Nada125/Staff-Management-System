import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ITask, TaskStatus } from '../../../Core/Models/Task';
import { TaskService } from '../../../Core/Services/task-service';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './task-form.html',
  styleUrls: ['./task-form.css'],
})
export class TaskForm implements OnInit {
  @Input() task?: ITask;

  taskStatuses = Object.values(TaskStatus);

  constructor(private _task: TaskService) {}

  myForm: FormGroup = new FormGroup({
    title: new FormControl('', Validators.required),
    description: new FormControl('', Validators.required),
    assignedDate: new FormControl(new Date().toISOString().slice(0, 16), Validators.required),
    dueDate: new FormControl(''),
    status: new FormControl(TaskStatus.InProgress, Validators.required),
    employeeId: new FormControl('', Validators.required),
  });

  ngOnInit(): void {
    if (this.task) {
      this.myForm.patchValue(this.task);
    }
  }

  onSubmit() {
    if (this.myForm.invalid) return;

    const formValue: ITask = {
      id: this.task?.id || 0,
      ...this.myForm.value,
    };

    if (this.task) {
      // Edit
      this._task.editTask(formValue.id, formValue).subscribe({
        next: (res) => console.log('Task updated:', res),
        error: (err) => console.error('Error updating task:', err),
      });
    } else {
      // Add
      this._task.addTask(formValue).subscribe({
        next: (res) => {
          console.log('Task added:', res);
          this.myForm.reset({
            assignedDate: new Date().toISOString().slice(0, 16),
            status: TaskStatus.InProgress,
          });
        },
        error: (err) => console.error('Error adding task:', err),
      });
    }
  }
}
