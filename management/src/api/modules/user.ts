import api from '../index'

export default {
  // 登录
  login: (data: {
    userName: string
    password: string
  }) => api.post('api/Account/SignInWithPassword', data),

  // 获取权限
  permission: () => api.get('api/management/GetUserInfo'),

  // 修改密码
  passwordEdit: (data: {
    password: string
    newpassword: string
  }) => api.post('user/password/edit', data, {
    baseURL: '/mock/',
  }),
  Query: (data: any) => api.get(`api/management/user`, { params: data }),
  Create: (data: any) => api.post('api/management/user', data),
  Modify: (data: any) => api.put('api/management/user', data),
  Delete: (data: string[]) => api.delete('api/management/user', { data: data }),
  Get: (id: string) => api.get(`api/management/user/${id}`),
  SaveRole: (data: any) => api.put(`api/management/user/Save/Roles`, data),
  GetUserBindRole: (id: string) => api.get(`api/management/user/Get/Roles/${id}`),
  GetUserInfo:()=>api.get(`api/management/user/GetUserInfo`)
}
