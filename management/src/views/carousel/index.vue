<template>
    <div>
        <pageMain>
            <el-row class="mb-4">
                <el-button type="primary" @click="Create">添加</el-button>
            </el-row>
            <el-table v-loading="table.loading" :data="table.data" stripe border style="width: 100%;margin-top:15px;"
                height="100%">
                <el-table-column prop="id" label="id" />
                <el-table-column prop="name" label="名称" />
                <el-table-column prop="filePath" label="图片" align="center">
                    <template #default="scope">
                        <el-image style="width: 100px; height: 100px" :src="file.getFile(scope.row.filePath)"
                            :zoom-rate="1.2" :preview-teleported="true"
                            :preview-src-list="[file.getFile(scope.row.filePath)]" :initial-index="0" fit="cover" />
                    </template>
                </el-table-column>
                <el-table-column prop="link" label="链接" />
                <el-table-column prop="sort" label="排序" align="center" />
                <el-table-column prop="dateCreated" label="创建日期" align="center">
                    <template #default="scope">
                        {{ moment.utc(scope.row.dateCreated).local().format("YYYY-MM-DD HH:mm") }}
                    </template>
                </el-table-column>
                <el-table-column label="操作" align="center">
                    <template #default="scope">
                        <el-row class="mb-4">
                            <el-button type="primary" @click="Modify(scope.row.id)" v-auth="'role.edit'">修改</el-button>
                            <el-button type="danger" v-auth="'role.delete'" @click="Delete([scope.row.id])">删除</el-button>
                        </el-row>
                    </template>
                </el-table-column>
            </el-table>
            <div style="margin-top: 30px; display: flex; justify-content: right;">
                <el-pagination background v-model:current-page="table.paging.page" v-model:page-size="table.paging.pageSize"
                    :page-sizes="table.pageArray" layout="total, sizes, prev, pager, next, jumper" :total="table.total"
                    @size-change="HandleSizeChange" @current-change="handleCurrentChange" />
            </div>
        </pageMain>
        <el-drawer v-model="dialog" title="数据操作" direction="rtl" :before-close="handleClose">
            <el-form label-position="top" label-width="100px" :model="formModel" :rules="rules" ref="carouselFormRef"
                @keyup.enter="onSubmit">
                <el-form-item label="名称" prop="name">
                    <el-input v-model="formModel.name" />
                </el-form-item>
                <el-form-item label="链接" prop="link">
                    <el-input v-model="formModel.link" />
                </el-form-item>
                <el-form-item label="排序" prop="sort">
                    <el-input v-model="formModel.sort" />
                </el-form-item>
                <el-form-item label="图片上传" prop="image">
                    <el-upload class="avatar-uploader" :before-upload="beforeAvatarUpload" :show-file-list="false">
                        <img v-if="formModel.image" :src="formModel.image" class="avatar" />
                        <el-icon v-else class="avatar-uploader-icon">
                            <Plus />
                        </el-icon>
                    </el-upload>
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
import { ElMessage, FormInstance, FormRules, UploadProps } from 'element-plus'
import api from '@/api/modules/carousel'
import file from '@/api/modules/file'
import moment from 'moment'
import { ref, reactive, onMounted } from 'vue'
import { Plus } from '@element-plus/icons-vue'
const table = reactive<any>({
    loading: false,
    data: [],
    pageArray: [20, 50, 100],
    total: 1,
    paging: {
        pageSize: 10,
        page: 1
    }
})
const isFold = false;
const dialog = ref(false)
const carouselFormRef = ref<FormInstance>()

const formModel = ref({
    id: '',
    name: '',
    link: '',
    sort: null,
    filePath: '',
    image: ''
})
const rules = reactive<FormRules>({
    name: [
        { required: true, message: '必填!', trigger: 'blur' },
    ],
})
const beforeAvatarUpload: UploadProps['beforeUpload'] = (rawFile) => {
    console.log('file', rawFile)
    file.upload(rawFile.name, rawFile).then((res: any) => {
        console.log(res)
        formModel.value.filePath = res
        formModel.value.image = file.getFile(res)
    })
    return false;
}

const HandleSizeChange = async (value: any) => {
    table.paging!.pageSize = value
    await query();
}
const handleCurrentChange = async (value: any) => {
    table.paging!.page = value
    await query();
}
onMounted(async () => {
    console.log('mounted')
    await query();
})

const query = async () => {
    table.loading = true;
    let queryParams = table.paging
    let res = (await api.Query(queryParams)) as any;
    table.data = res.data
    table.loading = false
    table.total = res.totalElements
    console.log(res)
    console.log(table.paging)
}

const handleClose = (done: () => void) => {
    carouselFormRef.value!.resetFields()
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
    if (res.filePath) {
        formModel.value.image = file.getFile(res.filePath)
    }
    console.log(id)
}
const Delete = async (ids: string[]) => {
    console.log(ids)
    await api.Delete(ids)
    await query();
    ElMessage.success('删除成功!')
}
const onSubmit = async () => {
    await carouselFormRef.value!.validate(async (valid, fields) => {
        if (valid) {
            if (formModel.value.id == '') {
                delete formModel.value!.id
                let res = await api.Create(formModel.value)
                carouselFormRef.value!.resetFields()
                formModel.value!.filePath = ''
                dialog.value = false
                ElMessage.success("添加成功!")
            } else {
                let res = await api.Modify(formModel.value)
                carouselFormRef.value!.resetFields()
                formModel.value!.filePath = ''
                dialog.value = false
                ElMessage.success("修改成功!")
            }
            formModel.value!.id = ''
            await query()
        }
    })

}
const cancel = () => {
    carouselFormRef.value!.resetFields()
    formModel.value!.filePath = ''
    dialog.value = false
}
</script>

<style scoped>
.avatar-uploader .avatar {
    width: 178px;
    height: 178px;
    display: block;
}
</style>

<style>
.avatar-uploader .el-upload {
    border: 1px dashed var(--el-border-color);
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
    transition: var(--el-transition-duration-fast);
}

.avatar-uploader .el-upload:hover {
    border-color: var(--el-color-primary);
}

.el-icon.avatar-uploader-icon {
    font-size: 28px;
    color: #8c939d;
    width: 178px;
    height: 178px;
    text-align: center;
}
</style>