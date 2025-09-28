import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TaskService } from '../../Core/Services/task-service';
import { ITask } from '../../Core/Models/Task';

@Component({
  selector: 'app-task',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tasks.html',
  styleUrls: ['./tasks.css'],
})
export class Task implements OnInit {
  tasks: ITask[] = [];

  constructor(private _task: TaskService, private router: Router) {}

  ngOnInit() {
    this.getTasks();
  }

  getTasks() {
    this._task.getTasks().subscribe((data) => {
      this.tasks = data;
    });
  }

  goToAdd() {
    this.router.navigate(['/dashboard/task/add']);
  }

  goToEdit(id: number) {
    this.router.navigate(['/dashboard/task/edit/', id]);
  }
}
