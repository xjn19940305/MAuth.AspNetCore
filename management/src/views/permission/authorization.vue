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
                    <el-tree ref="permissionTreeRef" :data="data" default-expand-all node-key="id" show-checkbox />
                </el-main>
            </el-container>

            <div style="margin-top: 10px;">
                <el-button type="primary" @click="save">保存</el-button>
            </div>
        </div>
    </pageMain>
</template>
      
<script lang="ts" setup>
import { asyncRoutes } from '@/router/routes'
import { RouteRecordRaw } from 'vue-router';
import { ElTree } from 'element-plus'
import { ref } from 'vue'
import type Node from 'element-plus/es/components/tree/src/model/node'
let data: any = ref([])
let roleData: any = ref([
    { id: 1, label: "超级管理员" },
    { id: 2, label: "普通角色" },
    { id: 3, label: "测试角色" },
    { id: 4, label: "无权限角色" }
])

const roleTreeRef = ref<InstanceType<typeof ElTree>>()
const permissionTreeRef = ref<InstanceType<typeof ElTree>>()

let roleClick = (dom: any) => {
    console.table(dom)

}

onMounted(() => {
    let dt = asyncRoutes.find(x => x.meta.title == '主菜单')
    if (dt != null) {
        let array: any = [];
        dt.children.forEach(item => {
            let obj = {
                label: item.meta.title,
                children: [],
            }
            if (item.meta.auth != null) {
                item.meta.auth.forEach(element => {
                    obj.children.push({ label: element })
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
            children: []
        }
        obj.children.push(temp)
        if (element.meta.auth != null) {
            element.meta.auth.forEach(child => {
                temp.children.push({
                    id: element.meta.title,
                    label: child
                })
            });
        }
        if (element.children != null) {
            recursion(element.children, temp)
        }
    });
}

const save = () => {

    // console.log(permissionTreeRef.value)
    console.log(permissionTreeRef.value!.getCheckedNodes(false, true))
}

</script>