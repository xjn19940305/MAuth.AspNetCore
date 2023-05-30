import type { RouteRecordRaw } from 'vue-router'

function Layout() {
    return import('@/layouts/index.vue')
}

const routes: RouteRecordRaw = {
    path: '/test',
    component: Layout,
    redirect: '/test/test1',
    name: 'test',
    meta: {
        title: '测试',
        icon: 'ep:lock',
    },
    children: [
        {
            path: 'test1',
            name: 'test1',
            component: () => import('@/views/test/test.vue'),
            meta: {
                title: '测试页面1',
                sidebar: false,
                breadcrumb: false,
                activeMenu: '/tm',
            },
        },
    ],
}

export default routes
