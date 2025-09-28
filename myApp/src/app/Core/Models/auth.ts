export interface IRegister {
  userName: string;
  phoneNumber: string;
  email: string;
  nationality: string;
  nationalId: string;
  password: string;
  confirmPassword: string;
}

export interface ILogin {
  email: string;
  password: string;
}

export interface ILoginRes {
  message: string;
  token: string;
}

export interface IUserData {
  id: string;
  role: string;
  name: string;
  exp: number;
}
