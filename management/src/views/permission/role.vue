<template>
    <div>
        <pageMain>
            <search-bar :fold="isFold">
                <template #default="{ fold }">
                    <el-form :model="queryFormModel" size="default" label-width="120px">
                        <el-row>
                            <el-col :span="12">
                                <el-form-item label="角色名称">
                                    <el-input v-model="queryFormModel.name" placeholder="请输入角色名称" clearable />
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <el-form-item>
                            <el-button type="primary" @click="query">
                                <template #icon>
                                    <el-icon>
                                        <svg-icon name="ep:search" />
                                    </el-icon>
                                </template>
                                查询
                            </el-button>
                        </el-form-item>
                    </el-form>
                </template>
            </search-bar>
        </pageMain>
        <pageMain>
            <el-row class="mb-4">
                <el-button type="primary" @click="Create">添加</el-button>
            </el-row>
            <el-table v-loading="table.loading" :data="table.data" stripe border style="width: 100%;margin-top:15px;"
                height="100%">
                <el-table-column prop="id" label="id" />
                <el-table-column prop="name" label="角色名称" />
                <el-table-column prop="description" label="描述" />
                <el-table-column prop="sort" label="排序" align="center" />
                <el-table-column prop="dateCreated" label="创建日期" align="center">
                    <template #default="scope">
                        {{ moment.utc(scope.row.dateCreated).local().format("YYYY-MM-DD HH:mm") }}
                    </template>
                </el-table-column>
                <el-table-column label="操作" align="center">
                    <template #default="scope">
                        <el-row class="mb-4">
                            <el-button type="primary" @click="Modify(scope.row.id)">修改</el-button>
                            <el-button type="danger" v-auth="'role.delete'" @click="Delete([scope.row.id])">删除</el-button>
                        </el-row>
                    </template>
                </el-table-column>
            </el-table>
            <div style="margin-top: 30px; display: flex; justify-content: right;">
                <el-pagination background v-model:current-page="paging.page" v-model:page-size="paging.pageSize"
                    :page-sizes="pageArray" layout="total, sizes, prev, pager, next, jumper" :total="total"
                    @size-change="HandleSizeChange" @current-change="handleCurrentChange" />
            </div>
        </pageMain>
        <el-drawer v-model="dialog" title="数据操作" direction="rtl" :before-close="handleClose">
            <el-form label-position="top" label-width="100px" :model="formModel" :rules="rules" ref="roleFormRef"
                @keyup.enter="onSubmit">
                <el-form-item label="角色名称" prop="name">
                    <el-input v-model="formModel.name" />
                </el-form-item>
                <el-form-item label="描述" prop="description">
                    <el-input v-model="formModel.description" />
                </el-form-item>
                <el-form-item label="排序" prop="sort">
                    <el-input v-model="formModel.sort" />
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="onSubmit">保存</el-button>
                    <el-button @click="cancel">取消</el-button>
                </el-form-item>
            </el-form>
        </el-drawer>

    </div>
</template>
  
<script lang="ts" setup>
import { ElMessage, FormInstance, FormRules } from 'element-plus'
import api from '@/api/modules/role'
import moment from 'moment'


const table = reactive<any>({
    loading: false,
    data: []
})
const isFold = false;
const dialog = ref(false)
const roleFormRef = ref<FormInstance>()
let queryFormModel = ref({
    name: ''
})
const formModel = ref({
    id: '',
    name: '',
    description: '',
    sort: null
})
const rules = reactive<FormRules>({
    name: [
        { required: true, message: '必填!', trigger: 'blur' },
    ],
})
const pageArray = [20, 50, 100]
const total = ref(1)
const paging: any = ref({
    pageSize: 10,
    page: 1
})
const HandleSizeChange = async (value: any) => {
    paging.value!.pageSize = value
    await query();
}
const handleCurrentChange = async (value: any) => {
    paging.value!.page = value
    await query();
}
onMounted(async () => {
    console.log('mounted')
    await query();
})

const query = async () => {
    table.loading = true;
    let queryParams = Object.assign(paging.value, queryFormModel.value)
    let res = (await api.Query(queryParams)) as any;
    table.data = res.data
    table.loading = false
    total.value = res.totalElements
    console.log(res)
    console.log(paging)
}

const handleClose = (done: () => void) => {
    roleFormRef.value!.resetFields()
    formModel.value!.id = ''
    done()
}
const Create = async () => {
    dialog.value = true
}
const Modify = async (id: string) => {
    dialog.value = true
    let res = (await api.Get(id)) as any
    formModel.value = res
    console.log(id)
}
const Delete = async (ids: string[]) => {
    console.log(ids)
    await api.Delete(ids)
    await query();
    ElMessage.success('删除成功!')
}
const onSubmit = async () => {
    await roleFormRef.value!.validate(async (valid, fields) => {
        if (valid) {
            if (formModel.value.id == '') {
                delete formModel.value!.id
                let res = await api.Create(formModel.value)
                roleFormRef.value!.resetFields()
                dialog.value = false
                ElMessage.success("添加成功!")
            } else {
                let res = await api.Modify(formModel.value)
                roleFormRef.value!.resetFields()
                dialog.value = false
                ElMessage.success("修改成功!")
            }
            formModel.value!.id = ''
            await query()
        }
    })

}
const cancel = () => {
    roleFormRef.value!.resetFields()
    dialog.value = false
}
</script>