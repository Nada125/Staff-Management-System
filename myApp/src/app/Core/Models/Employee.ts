import { ITask } from './Task';

export interface IEmployee {
  id: string;
  userName: string;
  email: string;
  phoneNumber: string;
  position: string;
  nationality: string;
  nationalId: string;
  salary: number;
  tasks?: ITask;
}
