import type { RouteRecordRaw } from 'vue-router'

function Layout() {
    return import('@/layouts/index.vue')
}

const routes: RouteRecordRaw = {
    path: '/permission',
    component: Layout,
    redirect: '/permission/authorization',
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
                sidebar: true,
                breadcrumb: true,
            },
        },
        {
            path: 'role',
            name: 'role',
            component: () => import('@/views/permission/role.vue'),
            meta: {
                title: '角色管理',
                sidebar: true,
                breadcrumb: true,
            },
        },
        {
            path: 'authorization',
            name: 'authorization',
            component: () => import('@/views/permission/authorization.vue'),
            meta: {
                title: '授权管理',
                sidebar: true,
                breadcrumb: true,
            },
        },
    ],
}

export default routes
