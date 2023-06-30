<template>
    <div>
        <pageMain>
            <el-row class="mb-4">
                <el-button type="primary" @click="Create" v-auth="'role.create'">添加</el-button>
            </el-row>
            <el-table v-loading="table.loading" :data="table.data" stripe border default-expand-all row-key="name"
                style="width: 100%;margin-top:15px;" height="100%">
                <!-- <el-table-column prop="id" label="id" width="400" /> -->
                <el-table-column prop="name" label="分类名称" />
                <el-table-column prop="sort" label="排序" align="center" />
                <el-table-column prop="picture" label="图片" align="center">
                    <template #default="scope">
                        <el-image style="width: 100px; height: 100px" :src="file.getFile(scope.row.picture)"
                            :zoom-rate="1.2" :preview-teleported="true"
                            :preview-src-list="[file.getFile(scope.row.picture)]" :initial-index="0" fit="cover" />
                    </template>
                </el-table-column>
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
            <el-form label-position="top" label-width="100px" :model="formModel" :rules="rules" ref="roleFormRef"
                @keyup.enter="onSubmit">
                <el-form-item label="分类名称" prop="name">
                    <el-input v-model="formModel.name" />
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
                <el-form-item label="父节点" prop="parentId">
                    <categorySelector v-model:parentId="formModel.parentId" />
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
import api from '@/api/modules/category'
import moment from 'moment'
import { reactive, ref, onMounted } from 'vue'
import file from '@/api/modules/file'
import categorySelector from '@/views/category/components/categorySelector.vue'
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
const roleFormRef = ref<FormInstance>()
const formModel = ref({
    id: '',
    name: '',
    picture: '',
    image: '',
    parentId: '',
    sort: null
})
const rules = reactive<FormRules>({
    name: [
        { required: true, message: '必填!', trigger: 'blur' },
    ],
    parentId: [
        { required: true, message: '必填!', trigger: 'blur' },
    ],
})

const HandleSizeChange = async (value: any) => {
    table.paging!.pageSize = value
    await query();
}
const handleCurrentChange = async (value: any) => {
    table.paging!.page = value
    await query();
}
onMounted(async () => {
    await query();
})
const beforeAvatarUpload: UploadProps['beforeUpload'] = (rawFile) => {
    console.log('file', rawFile)
    file.upload(rawFile.name, rawFile).then((res: any) => {
        console.log(res)
        formModel.value.picture = res
        formModel.value.image = file.getFile(res)
    })
    return false;
}

const query = async () => {
    table.loading = true;
    table.data = []
    let res = (await api.GetAllCategories()) as any;
    table.loading = false
    // table.total = res.totalElements
    ToTreeList(res, table.data, "0");
    console.log(res)
    console.log(table.paging)
}
const ToTreeList = (list: any, tree: any, parentId: any) => {
    list.forEach((item: any) => {
        // 判断是否为父级菜单
        if (item.parentId === parentId) {
            item.meta = item.meta || {};
            const child = {
                ...item,
                id: item.id,
                name: item.name,
                parentId: item.parentId,
                dateCreated: item.dateCreated,
                picture: item.picture,
                children: [],
            };
            // 迭代 list， 找到当前菜单相符合的所有子菜单
            ToTreeList(list, child.children, item.id);
            // 删掉不存在 children 值的属性
            if (child.children.length <= 0) {
                delete child.children;
            }
            // 加入到树中
            tree.push(child);
        }
    });
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
    if (res.picture) {
        formModel.value.image = file.getFile(res.picture)
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
    await roleFormRef.value!.validate(async (valid, fields) => {
        if (valid) {
            formModel.value.parentId = formModel.value.parentId || '0'
            if (formModel.value.id == '') {
                delete formModel.value!.id
                let res = await api.Create(formModel.value)
                roleFormRef.value!.resetFields()
                formModel.value!.picture = ''
                dialog.value = false
                ElMessage.success("添加成功!")
            } else {
                let res = await api.Modify(formModel.value)
                roleFormRef.value!.resetFields()
                formModel.value!.picture = ''
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