export interface ITask {
  id: number;
  title: string;
  description: string;
  assignedDate: string;
  dueDate?: string;
  status: TaskStatus;
  employeeId: string;
}

export enum TaskStatus {
  Completed = 'Completed',
  InProgress = 'InProgress',
  NeedReview = 'NeedReview',
}
