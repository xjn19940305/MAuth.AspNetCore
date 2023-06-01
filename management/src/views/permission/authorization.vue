<route lang="yaml">
    meta:
      enabled: false
    </route>
    
<template>
    <pageMain>
        <div>
            <el-container>
                <el-aside width="200px">
                    <el-tree ref="roleTreeRef" :data="roleData" default-expand-all node-key="id"
                        @node-click="roleClick" /></el-aside>
                <el-main>
                    <el-tree ref="permissionTreeRef" :data="data" default-expand-all node-key="label" show-checkbox />
                </el-main>
            </el-container>

            <div style="margin-top: 10px;">
                <el-button type="primary" @click="save" :disabled="disabled">保存</el-button>
            </div>
        </div>
    </pageMain>
</template>
      
<script lang="ts" setup>
import { asyncRoutes } from '@/router/routes'
import { RouteRecordRaw } from 'vue-router';
import { ElMessage, ElTree } from 'element-plus'
import { ref } from 'vue'
import roleApi from '@/api/modules/role'
import type Node from 'element-plus/es/components/tree/src/model/node'
let data: any = ref([])
let roleData: any = ref([
    { id: 1, label: "超级管理员" },
    { id: 2, label: "普通角色" },
    { id: 3, label: "测试角色" },
    { id: 4, label: "无权限角色" }
])
let role: any = ref({})
let disabled = ref(true)
const roleTreeRef = ref<InstanceType<typeof ElTree>>()
const permissionTreeRef = ref<InstanceType<typeof ElTree>>()

let roleClick = async (dom: any) => {
    console.table(dom)
    role.value = dom
    disabled.value = false
    let res = (await roleApi.GetRoleAuthorization(dom.id)) as any
    console.log(res)
    permissionTreeRef.value!.setCheckedNodes(res.map(f => { return { label: f } }));
}
const loadRole = async () => {
    let res = (await roleApi.Query({ page: 1, pageSize: 9999 })) as any
    roleData.value = res.data.map(f => {
        return { id: f.id, label: f.name }
    })
}
onMounted(async () => {
    await loadRole();
    let dt = asyncRoutes.find(x => x.meta.title == '主菜单')
    if (dt != null) {
        let array: any = [];
        dt.children.forEach(item => {
            let obj = {
                label: item.meta.title,
                children: [],
                isAuth: false
            }
            if (item.meta.auth != null) {
                item.meta.auth.forEach(element => {
                    obj.children.push({ label: element, isAuth: true })
                });
            }
            array.push(obj)
            if (item.children != null) {
                recursion(item.children, obj)
            }
        })
        data.value = array
        console.log(data)
    }
})
1
const recursion = (res: RouteRecordRaw[], obj: any) => {
    res.forEach(element => {
        let temp = {
            id: element.meta.title,
            label: element.meta.title,
            children: [],
            isAuth: false
        }
        obj.children.push(temp)
        if (element.meta.auth != null) {
            element.meta.auth.forEach(child => {
                temp.children.push({
                    id: new Date().toLocaleString(),
                    label: child,
                    isAuth: true
                })
            });
        }
        if (element.children != null) {
            recursion(element.children, temp)
        }
    });
}

const save = async () => {

    // console.log(permissionTreeRef.value)
    let selectedAuth = permissionTreeRef.value!.getCheckedNodes(false, true).filter(x => x.isAuth)
    console.log(selectedAuth)
    console.log(role.value.id)
    let res = await roleApi.Authorization(role.value.id, selectedAuth.map(x => x.label))
    ElMessage.success('保存成功!')
}

</script>