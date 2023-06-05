import type { RouteRecordRaw } from 'vue-router'

function Layout() {
    return import('@/layouts/index.vue')
}

const routes: RouteRecordRaw = {
    path: '/permission',
    component: Layout,
    redirect: '/permission/user',
    name: 'permission',
    meta: {
        title: '权限管理',
        icon: 'ep:lock',
    },
    children: [
        {
            path: 'user',
            name: 'user',
            component: () => import('@/views/permission/user.vue'),
            meta: {
                title: '用户管理',
                auth: ['user.create', 'user.edit', 'user.delete', 'user.bindrole', 'user.search']
            },
        },
        {
            path: 'role',
            name: 'role',
            component: () => import('@/views/permission/role.vue'),
            meta: {
                title: '角色管理',
                auth: ['role.query', 'role.create', 'role.edit', 'role.delete', 'role.search']
            },
        },
        {
            path: 'authorization',
            name: 'authorization',
            component: () => import('@/views/permission/authorization.vue'),
            meta: {
                title: '授权管理',
                auth: ['authorization.create', 'authorization.edit', 'authorization.delete']
            },
        },
    ],
}

export default routes
